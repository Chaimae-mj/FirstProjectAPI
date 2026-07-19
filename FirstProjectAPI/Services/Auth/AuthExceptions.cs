namespace FirstProjectAPI.Services
{
    public class EmailAlreadyExistsException : Exception
    {
        public EmailAlreadyExistsException() : base("Un compte existe déjà avec cet email.") { }
    }

    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("Email ou mot de passe incorrect.") { }
    }

    public class InvalidStaffRoleException : Exception
    {
        public InvalidStaffRoleException() : base("Le rôle doit être 'Formateur' ou 'Admin' pour cet endpoint.") { }
    }
}
