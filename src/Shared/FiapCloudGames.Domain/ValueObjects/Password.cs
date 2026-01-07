namespace FiapCloudGames.Domain.ValueObjects
{
    public class Password
    {
        public string Value { get; private set; }

        public Password(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Senha não pode ser vazia", nameof(value));

            if (value.Length < 8)
                throw new ArgumentException("Senha deve ter no mínimo 8 caracteres", nameof(value));

            if (!value.Any(char.IsUpper))
                throw new ArgumentException("Senha deve ter pelo menos uma letra maiúscula", nameof(value));

            if (!value.Any(char.IsLower))
                throw new ArgumentException("Senha deve ter pelo menos uma letra minúscula", nameof(value));

            if (!value.Any(char.IsDigit))
                throw new ArgumentException("Senha deve ter pelo menos um número", nameof(value));

            if (!value.Any(c => !char.IsLetterOrDigit(c)))
                throw new ArgumentException("Senha deve ter pelo menos um caractere especial", nameof(value));

            Value = value;
        }

        public static implicit operator string(Password password) => password.Value;
        public static implicit operator Password(string value) => new(value);

        public override string ToString() => "********";
    }
}
