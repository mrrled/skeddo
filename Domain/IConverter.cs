namespace Domain;

public interface IConverter<in TFrom, out TTo>
{
    public static abstract TTo Convert(TFrom from);
}