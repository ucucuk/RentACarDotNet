﻿using Domain.Entities;

namespace RentACarDotNetCore.Application.DTOs
{
    public class ModelDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public BrandDTO Brand { get; set; }
    }
}
