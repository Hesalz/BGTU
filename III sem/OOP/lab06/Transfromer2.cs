namespace lab05
{
    partial class Transformer
    {
        public override string ToString()
        {
            return $"Трансформер: {Name}, Модель: {Model}, Год создания: {Year}, {Specs}";
        }

        public void CheckOverload(int maxPower)
        {
            if (Specs.PowerLevel > maxPower)
            {
                throw new TransformerOverloadException($"Мощность {Specs.PowerLevel} превышает допустимый предел {maxPower}.");
            }
        }
    }
}
