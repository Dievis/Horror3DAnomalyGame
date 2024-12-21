using UnityEngine;

public static class TransformExtensions
{
    public static Transform FindRecursive(this Transform parent, string name)
    {
        // Kiểm tra nếu Transform hiện tại có tên trùng khớp
        if (parent.name == name)
        {
            return parent;
        }

        // Duyệt qua các Transform con
        foreach (Transform child in parent)
        {
            Transform result = child.FindRecursive(name);
            if (result != null)
            {
                return result;
            }
        }

        // Không tìm thấy
        return null;
    }
}

