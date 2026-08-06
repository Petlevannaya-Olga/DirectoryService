namespace DirectoryService.Contracts.Locations;

public record LocationAddressDto(
    string PostalCode,
    string Region,
    string City,
    string Street,
    int House,
    int? Apartment);