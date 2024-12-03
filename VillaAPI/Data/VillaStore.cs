
using System;
using VillaAPI.Models.Dto;

namespace VillaAPI.Data
{
	public class VillaStore
	{
		public static List<VillaDto> VillaList = new List<VillaDto> {
                new VillaDto {Id= 1, Name="Poll View", Sqft=100, Occupancy=4},
                new VillaDto {Id= 2, Name="Beach View", Sqft=200, Occupancy=2},
                new VillaDto {Id= 3, Name="Without View", Sqft=140, Occupancy=3},
        };

	}
}

