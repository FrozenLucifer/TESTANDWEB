namespace DTOs;

public record LoginRequestDto(string Username, string Password);

public record LoginResponseDto(string Token);

public record ChangePasswordRequestDto(string Username, string OldPassword, string NewPassword);
