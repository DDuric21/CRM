using Backend_API.Data.DbContext;
using Backend_API.Data.Models;
using Backend_API.Logging;
using Microsoft.EntityFrameworkCore;
using Models.Helpers;

namespace Backend_API.Data.Repositories
{
    public class CustomerAssetsRepository : GenericRepository<CustomerAssets>, ICustomerAssetsRepository
    {
        public CustomerAssetsRepository(CrmDbContext context) : base(context)
        {
        }

        public async Task<int> UpdateCustomerAssetDataAsync(CustomerAssets updatedAsset)
        {
            var existingCustomerAsset = _context.CustomerAssets
                .Include(x => x.CustomerAssetOptions)
                .ThenInclude(x => x.Option)
                .SingleOrDefault(x => x.Id == updatedAsset.Id);

            if (existingCustomerAsset == null)
            {
                return 0;
            }

            try
            {
                _context.Entry(existingCustomerAsset).CurrentValues.SetValues(updatedAsset);
                _context.Entry(existingCustomerAsset).State = EntityState.Modified;

                UpdateCustomerAssetOption(updatedAsset, existingCustomerAsset);

                var result = await SaveAsync();

                return result;
            }
            catch (Exception ex)
            {
                DynamicLogger.LogException(ex, ex.Message);
                return 0;
            }
        }

        private void UpdateCustomerAssetOption(CustomerAssets updatedAsset, CustomerAssets existingCustomerAsset)
        {
            var updatedAssetOptions = updatedAsset.CustomerAssetOptions ?? new List<CustomerAssetOptions>();
            var existingAssetOptions = existingCustomerAsset.CustomerAssetOptions ?? new List<CustomerAssetOptions>();

            // I believe some of this should be in service and not here
            UpdateAssetOptions(updatedAssetOptions, existingAssetOptions);
        }

        private void UpdateAssetOptions(ICollection<CustomerAssetOptions> updatedAssetOptions, ICollection<CustomerAssetOptions> existingAssetOptions)
        {
            RemoveDeletedOptions(updatedAssetOptions, existingAssetOptions);

            //Currently nothing to update
            //UpdateExistingOptions(updatedAssetOptions, existingAssetOptions);

            AddNewOptions(updatedAssetOptions, existingAssetOptions);
        }

        private void RemoveDeletedOptions(ICollection<CustomerAssetOptions> updatedAssetOptions, ICollection<CustomerAssetOptions> existingAssetOptions)
        {
            var deletedOptions = existingAssetOptions
                .Where(x => !updatedAssetOptions.Select(y => y.OptionID).Contains(x.OptionID))
                .ToList();

            if (!deletedOptions.IsNullOrEmpty())
            {
                _context.CustomerAssetOptions.RemoveRange(deletedOptions);
            }
        }

        private void AddNewOptions(ICollection<CustomerAssetOptions> updatedAssetOptions, ICollection<CustomerAssetOptions> existingAssetOptions)
        {
            var newOptions = updatedAssetOptions
                .Where(x => !existingAssetOptions.Select(y => y.OptionID).Contains(x.OptionID))
                .Select(x => new CustomerAssetOptions
                {
                    CustomerAssetsID = x.CustomerAssetsID,
                    OptionID = x.OptionID
                })
                .ToList();

            if (!newOptions.IsNullOrEmpty())
            {
                _context.CustomerAssetOptions.AddRange(newOptions);
            }
        }

        private void UpdateExistingOptions(ICollection<CustomerAssetOptions> updatedAssetOptions, ICollection<CustomerAssetOptions> existingAssetOptions)
        {
            var updatedOptions = existingAssetOptions
                .Where(x => updatedAssetOptions.Select(y => y.OptionID).Contains(x.OptionID))
                .ToList();

            if (!updatedOptions.IsNullOrEmpty())
            {
                foreach (var existingAssetOption in updatedOptions)
                {
                    var updatedAssetOption = updatedAssetOptions.First(x => x.OptionID == existingAssetOption.OptionID);
                    _context.Entry(existingAssetOption).CurrentValues.SetValues(updatedAssetOption);
                    _context.Entry(existingAssetOption).State = EntityState.Modified;
                }
            }
        }
    }
}
