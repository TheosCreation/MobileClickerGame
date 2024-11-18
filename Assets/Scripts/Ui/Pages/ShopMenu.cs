public class ShopMenu : UiPage
{
    public void PurchaseNoAds()
    {
        PurchaseManager.Instance.PurchaseItem(PurchaseManager.productId_NoAds);
    }

    public void Purchase1000Money()
    {
        PurchaseManager.Instance.PurchaseItem(PurchaseManager.productId_Money1000);
    }

    public void Back()
    {
        UiManager.Instance.CloseShopMenu();
    }
}