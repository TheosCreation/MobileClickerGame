using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class PurchaseManager : MonoBehaviour, IDetailedStoreListener
{
    public static PurchaseManager Instance { get; private set; }
    public bool m_initialised { get; private set; }
    const string productId_Money1000 = "money_1000";
    const string productId_NoAds = "no_ads";

    private IStoreController storeController;
    private IExtensionProvider storeExtensionProvider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        m_initialised = false;
    }

    public IEnumerator Init()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(productId_Money1000, ProductType.Consumable);
        builder.AddProduct(productId_NoAds, ProductType.NonConsumable);

        var serv = GetComponent<UnityServicesInit>();
        while (serv.m_initialised == false)
        {
            yield return null;
        }

        Debug.Log("Initializing purchasing");
        UnityPurchasing.Initialize(this, builder);
    }

    public void PurchaseItem(string productId)
    {
        if (m_initialised && storeController != null)
        {
            storeController.InitiatePurchase(productId);
            Debug.Log($"Attempting to purchase product: {productId}");
        }
        else
        {
            Debug.LogWarning("Purchase failed: Purchasing not initialized or storeController is null.");
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"Purchase Failed for product: [{product.definition.id}] - Reason: {failureDescription.reason}");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"Purchase Manager Init failed: {error}");
        m_initialised = false;
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log($"Purchase Manager Init failed: {error} - {message}");
        m_initialised = false;
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        switch (purchaseEvent.purchasedProduct.definition.id)
        {
            case productId_Money1000:
                Debug.Log("Successfully purchased Money 1000!");
                GameManager.Instance.CurrentMoney += 1000;
                return PurchaseProcessingResult.Complete;

            case productId_NoAds:
                Debug.Log("Successfully purchased No Ads!");
                AdManager.Instance.DisableAds();
                return PurchaseProcessingResult.Complete;

            default:
                Debug.LogWarning($"Unknown product ID: {purchaseEvent.purchasedProduct.definition.id}");
                return PurchaseProcessingResult.Pending;
        }
    }


    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        storeExtensionProvider = extensions;
        m_initialised = true;
        Debug.Log("Purchase Manager initialized successfully");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // Log the product ID and the reason for the failure
        Debug.LogError($"Purchase failed for product: {product.definition.id}. Reason: {failureReason}");

        // Handle specific failure reasons if needed
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
}