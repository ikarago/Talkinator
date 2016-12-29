using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talkinator.Services;
using Windows.Services.Store;

namespace Talkinator.Helpers
{
    static public class LicenseHelper
    {
        // Check License status
        public static async Task<bool> CheckPurchaseStatus()
        {
            StoreContext store = StoreContext.GetDefault();

            // Donation status is false by default
            bool purchased = false;

            // Check for donation
            // Permanent Ad-free
            string[] productKinds = { "Durable" };
            List<string> filterList = new List<string>(productKinds);
            StoreProductQueryResult result = await store.GetUserCollectionAsync(filterList);

            if (result != null)
            {
                List<StoreProduct> products = new List<StoreProduct>();
                foreach (var item in result.Products)
                {
                    StoreProduct prod = item.Value;
                    products.Add(prod);
                }

                foreach (var item in products)
                {
                    if (item.IsInUserCollection == true)
                    {
                        purchased = true;
                        Debug.WriteLine("LicenseHelper - Ad-free active");
                    }
                }
            }
            //if (store.GetAssociatedStoreProductsAsync().)
            //if (licenseInformation.ProductLicenses[adfreePermanent].IsActive)
            //{
            //    
            //    purchased = true;
            //}
            //// Ad-free Tier 1
            //else if (licenseInformation.ProductLicenses[adfreeTier1].IsActive)
            //{
            //    Debug.WriteLine("IAP: Ad-free T1 active");
            //    purchased = true;
            //}

            return purchased;
        }

        public static async Task Purchase(int tier)
        {
            // Permanent Ad-free
            if (tier == 0)
            {
                await PurchaseAddOn("9nblggh4tn7h");
            }
            // 90 days ad-free
            else if (tier == 1)
            {
                await PurchaseAddOn("9nblggh4tg5q");
            }
        }

        private static async Task<bool> PurchaseAddOn(string storeId)
        {
            bool success = false;
            StoreContext store = StoreContext.GetDefault();

            StorePurchaseResult result = await store.RequestPurchaseAsync(storeId);

            switch (result.Status)
            {
                case StorePurchaseStatus.AlreadyPurchased:
                {
                    Debug.WriteLine("LicenseHelper - AddOn already purchased. :D StoreId = " + storeId);
                    MessageHelper.ShowIapAlreadyBoughtMessage();
                    break;
                }
                case StorePurchaseStatus.Succeeded:
                {
                    Debug.WriteLine("LicenseHelper - AddOn purchased. :) You are awesome! StoreId = " + storeId);
                    success = true;
                    MessageHelper.ShowIapAcquisitionSuccessfulMessage();
                    break;
                }
                case StorePurchaseStatus.NotPurchased:
                {
                    Debug.WriteLine("LicenseHelper - AddOn NOT purchased. :( StoreId = " + storeId);
                    MessageHelper.ShowIapAcquisitionFailedMessage();
                    break;
                }
                case StorePurchaseStatus.NetworkError:
                {
                    Debug.WriteLine("LicenseHelper - Network Error. :/ StoreId = " + storeId);
                    MessageHelper.ShowIapAcquisitionFailedMessage();
                    break;
                }
                case StorePurchaseStatus.ServerError:
                {
                    Debug.WriteLine("LicenseHelper - Server Error. :/ StoreId = " + storeId);
                    MessageHelper.ShowIapAcquisitionFailedMessage();
                    break;
                }
                default:
                {
                    Debug.WriteLine("LicenseHelper - Unknown Error. :/ StoreId = " + storeId);
                    MessageHelper.ShowIapAcquisitionFailedMessage();
                    break;
                }
            }

            return success;
        }
    }
}
