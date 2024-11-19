using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class PurchaseManager : MonoBehaviour, IDetailedStoreListener
{
    // Singleton instance to ensure a single PurchaseManager across scenes
    public static PurchaseManager Instance { get; private set; }

    // Indicates whether the purchasing system is initialized
    public bool m_initialised { get; private set; }

    // Product IDs for in-app purchases
    public const string productId_Money1000 = "money_1000";
    public const string productId_NoAds = "no_ads";

    // Store controller and extension provider for handling purchases
    private IStoreController storeController;
    private IExtensionProvider storeExtensionProvider;

    private void Awake()
    {
        // Set up the singleton instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }

        m_initialised = false;
    }

    /// <summary>
    /// Initializes the purchasing system asynchronously.
    /// </summary>
    public IEnumerator Init()
    {
        // Create the configuration builder for products
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(productId_Money1000, ProductType.Consumable); // Consumable product
        builder.AddProduct(productId_NoAds, ProductType.NonConsumable); // Non-consumable product

        // Wait for Unity Services initialization
        var serv = GetComponent<UnityServicesInit>();
        while (serv.m_initialised == false)
        {
            yield return null;
        }

        Debug.Log("Initializing purchasing");
        UnityPurchasing.Initialize(this, builder); // Initialize Unity Purchasing

        ApplyNonConsumablePurchases(); // Apply previously purchased non-consumable items
    }

    /// <summary>
    /// Initiates a purchase for the specified product ID.
    /// </summary>
    public void PurchaseItem(string productId)
    {
        if (m_initialised && storeController != null)
        {
            // Handle No Ads product specifically
            if (productId == productId_NoAds)
            {
                Product noAdsProduct = storeController.products.WithID(productId_NoAds);

                // Check if No Ads has already been purchased
                if (noAdsProduct != null && noAdsProduct.hasReceipt)
                {
                    Debug.Log("No Ads has already been purchased based on the receipt. Applying it again.");
                    AdManager.Instance.DisableAds(); // Apply No Ads feature
                    return; // Skip initiating a new purchase
                }
            }

            // Proceed with the purchase if no receipt is found
            storeController.InitiatePurchase(productId);
            Debug.Log($"Attempting to purchase product: {productId}");
        }
        else
        {
            Debug.LogWarning("Purchase failed: Purchasing not initialized or storeController is null.");
        }
    }

    /// <summary>
    /// Callback for handling purchase failures.
    /// </summary>
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"Purchase Failed for product: [{product.definition.id}] - Reason: {failureDescription.reason}");
    }

    /// <summary>
    /// Callback for handling initialization failures.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"Purchase Manager Init failed: {error}");
        m_initialised = false;
    }

    /// <summary>
    /// Callback for handling initialization failures with a message.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log($"Purchase Manager Init failed: {error} - {message}");
        m_initialised = false;
    }

    /// <summary>
    /// Processes a successful purchase.
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        switch (purchaseEvent.purchasedProduct.definition.id)
        {
            case productId_Money1000:
                Debug.Log("Successfully purchased Money 1000!");
                GameManager.Instance.CurrentMoney += 1000; // Add in-game currency
                return PurchaseProcessingResult.Complete;

            case productId_NoAds:
                Debug.Log("Successfully purchased No Ads!");
                AdManager.Instance.DisableAds(); // Disable ads
                return PurchaseProcessingResult.Complete;

            default:
                Debug.LogWarning($"Unknown product ID: {purchaseEvent.purchasedProduct.definition.id}");
                return PurchaseProcessingResult.Pending; // Keep pending if unhandled
        }
    }

    /// <summary>
    /// Callback for successful initialization of the purchasing system.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        storeExtensionProvider = extensions;
        m_initialised = true;
        Debug.Log("Purchase Manager initialized successfully");
    }

    /// <summary>
    /// Handles specific reasons for purchase failure.
    /// </summary>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"Purchase failed for product: {product.definition.id}. Reason: {failureReason}");

        // Log detailed failure reasons
        switch (failureReason)
        {
            case PurchaseFailureReason.PurchasingUnavailable:
                Debug.LogError("Purchasing is unavailable on this device.");
                break;
            case PurchaseFailureReason.ExistingPurchasePending:
                Debug.LogError("A previous purchase is still pending.");
                break;
            case PurchaseFailureReason.ProductUnavailable:
                Debug.LogError("The requested product is not available in the store.");
                break;
            case PurchaseFailureReason.SignatureInvalid:
                Debug.LogError("The purchase signature is invalid.");
                break;
            case PurchaseFailureReason.UserCancelled:
                Debug.LogWarning("The user cancelled the purchase.");
                break;
            case PurchaseFailureReason.PaymentDeclined:
                Debug.LogError("The payment was declined.");
                break;
            case PurchaseFailureReason.DuplicateTransaction:
                Debug.LogWarning("This transaction is a duplicate.");
                break;
            default:
                Debug.LogError("Purchase failed due to an unknown error.");
                break;
        }
    }

    /// <summary>
    /// Applies previously purchased non-consumable items like No Ads.
    /// </summary>
    private void ApplyNonConsumablePurchases()
    {
        // Check if the No Ads product has a receipt
        Product noAdsProduct = storeController.products.WithID(productId_NoAds);

        if (noAdsProduct != null && noAdsProduct.hasReceipt)
        {
            Debug.Log("Applying previously purchased No Ads.");
            AdManager.Instance.DisableAds(); // Disable ads if No Ads is purchased
        }
    }
}