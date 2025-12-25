namespace DirectoryService.Contracts;

public record AddressDto(
    string PostalCode,
    string Region,
    string City,
    string Street,
    int House,
    int? Apartment);