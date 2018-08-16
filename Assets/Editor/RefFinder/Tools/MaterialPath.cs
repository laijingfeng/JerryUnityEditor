public class MaterialPath : Common_GuidPath
{
    protected override string GetSupportInfoExt()
    {
        string ext = "模型里尝试GUID查找会报错，先不查模型";
        if (string.IsNullOrEmpty(base.GetSupportInfoExt()))
        {
            return ext;
        }
        return  string.Format("{0},{1}", base.GetSupportInfoExt(), ext);
    }
}