using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class PurchaseManager : MonoBehaviour, IDetailedStoreListener
{
    public static PurchaseManager Instance { get; private set; }
    public bool m_initialised { get; private set; }
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
        //builder = builder.AddProduct(productId_Gems, ProductType.Consumable);

        var serv = GetComponent<UnityServicesInit>();
        while(serv.m_initialised == false)
        {
            yield return null;
        }

        Debug.Log("Initialising purchasing");
        UnityPurchasing.Initialize(this, builder);
    }

    public void PurchaseItem(string _productID)
    {
        
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"Purchase Failed for product: [{product.transactionID}]");
    }
    
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase Failed for product: [{product.transactionID}]");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Purchase Manager Init failed");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("Purchase Manager Init failed");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        throw new System.NotImplementedException();
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("Purchase Manager Init success");
    }
}
