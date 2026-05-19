using Models.DTO;

namespace Models.Helpers
{
    public class CustomerAssetsSorter : IComparer<AssetDTO>
    {
        public int Compare(AssetDTO x, AssetDTO y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentException("Arguments cannot be null");
            }

            var isGreater = (int)x.AssetStatus < (int)y.AssetStatus
                ? -1
                : x.CustomerAssetID.CompareTo(y.CustomerAssetID);

            return isGreater;
        }
    }
}
