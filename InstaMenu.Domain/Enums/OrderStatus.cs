namespace InstaMenu.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 0,            // في الانتظار (الطلب تم إنشاؤه فقط)
        Processing = 1,         // قيد المعالجة (يتم تجهيزه داخل المطعم)
        OnTheWay = 2,           // في الطريق للوصول (الطلب خرج للتوصيل)
        Cancelled = 3,          // ملغي
        Completed = 4           // تم التوصيل والانتهاء من الطلب
    }

}
