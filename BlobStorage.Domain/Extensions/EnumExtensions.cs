namespace BlobStorage.Domain.Extensions
{
	public static class EnumExtensions
	{
		public static T ToEnum<T>(this string value)
		{
			return (T)System.Enum.Parse(typeof(T), value, true);
		}
	}
}
