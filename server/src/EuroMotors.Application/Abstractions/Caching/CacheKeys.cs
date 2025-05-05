namespace EuroMotors.Application.Abstractions.Caching;

public static class CacheKeys
{
    public static class Products
    {
        private const string Prefix = "products";

        public static string GetList() => $"{Prefix}:list";
        public static string GetById(Guid id) => $"{Prefix}:id:{id}";
        public static string GetByCategory(Guid categoryId) => $"{Prefix}:category:{categoryId}";
        public static string GetByBrandName(string brandName) => $"{Prefix}:brand:{brandName}";
        public static string GetAllPrefix() => Prefix;
    }

    public static class Categories
    {
        private const string Prefix = "categories";

        public static string GetList() => $"{Prefix}:list";
        public static string GetHierarchical() => $"{Prefix}:hierarchical";
        public static string GetById(Guid id) => $"{Prefix}:id:{id}";
        public static string GetAllPrefix() => Prefix;
    }

    public static class CarBrands
    {
        private const string Prefix = "car-brands";

        public static string GetList() => $"{Prefix}:list";
        public static string GetById(Guid id) => $"{Prefix}:id:{id}";
        public static string GetAllPrefix() => Prefix;
        public static string GetAllForModels() => $"{GetList()}:all";
    }

    public static class CarModels
    {
        private const string Prefix = "car-models";

        public static string GetList() => $"{Prefix}:list";
        public static string GetById(Guid id) => $"{Prefix}:id:{id}";
        public static string GetByBrandId(Guid brandId) => $"{Prefix}:brand:{brandId}";
        public static string GetSelection() => $"{Prefix}:selection";
        public static string GetAllPrefix() => Prefix;
    }
}