using BaGet.Core;
using CleanMicroserviceSystem.Astra.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CleanMicroserviceSystem.Astra.WebAPI.Controllers;

[ApiController]
public class SymbolController : ControllerBase
{
    private readonly IAuthenticationService _authentication;
    private readonly ISymbolIndexingService _indexer;
    private readonly ISymbolStorageService _storage;
    private readonly IOptionsSnapshot<BaGetOptions> _options;
    private readonly ILogger<SymbolController> _logger;

    public SymbolController(
        IAuthenticationService authentication,
        ISymbolIndexingService indexer,
        ISymbolStorageService storage,
        IOptionsSnapshot<BaGetOptions> options,
        ILogger<SymbolController> logger)
    {
        this._authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
        this._indexer = indexer ?? throw new ArgumentNullException(nameof(indexer));
        this._storage = storage ?? throw new ArgumentNullException(nameof(storage));
        this._options = options ?? throw new ArgumentNullException(nameof(options));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPut]
    [Route("api/v2/symbol")]
    public async Task Upload(CancellationToken cancellationToken)
    {
        if (this._options.Value.IsReadOnlyMode || !await this._authentication.AuthenticateAsync(this.Request.GetApiKey(), cancellationToken))
        {
            this.HttpContext.Response.StatusCode = 401;
            return;
        }

        try
        {
            using var uploadStream = await this.Request.GetUploadStreamOrNullAsync(cancellationToken);
            if (uploadStream == null)
            {
                this.HttpContext.Response.StatusCode = 400;
                return;
            }

            var result = await this._indexer.IndexAsync(uploadStream, cancellationToken);

            switch (result)
            {
                case SymbolIndexingResult.InvalidSymbolPackage:
                    this.HttpContext.Response.StatusCode = 400;
                    break;

                case SymbolIndexingResult.PackageNotFound:
                    this.HttpContext.Response.StatusCode = 404;
                    break;

                case SymbolIndexingResult.Success:
                    this.HttpContext.Response.StatusCode = 201;
                    break;
            }
        }
        catch (Exception e)
        {
            this._logger.LogError(e, "Exception thrown during symbol upload");

            this.HttpContext.Response.StatusCode = 500;
        }
    }

    [HttpGet]
    [Route("api/download/symbols/{file}/{key}/{file2}")]
    public async Task<IActionResult> Get(string file, string key)
    {
        var pdbStream = await this._storage.GetPortablePdbContentStreamOrNullAsync(file, key);
        return pdbStream == null ? this.NotFound() : this.File(pdbStream, "application/octet-stream");
    }
}
