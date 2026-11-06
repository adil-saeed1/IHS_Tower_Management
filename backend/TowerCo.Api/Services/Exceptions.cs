namespace TowerCo.Api.Services;

public class NotFoundException(string message) : Exception(message);

// A request that is valid in shape but breaks a business rule (e.g. tower is full).
public class BusinessRuleException(string message) : Exception(message);
