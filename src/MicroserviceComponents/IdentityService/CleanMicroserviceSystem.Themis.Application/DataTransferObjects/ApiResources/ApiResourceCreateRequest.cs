﻿using System.ComponentModel.DataAnnotations;

namespace CleanMicroserviceSystem.Themis.Application.DataTransferObjects.ApiResources
{
    public class ApiResourceCreateRequest
    {
        [Required(ErrorMessage = "Api resource name is required")]
        public string Name { get; set; }

        public bool Enabled { get; set; }

        public string? Description { get; set; }
    }
}
