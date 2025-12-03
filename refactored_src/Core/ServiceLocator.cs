using System;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************
 *  Refactored with: Service Locator Pattern
 *  Pattern Type: Additional
 *
 *  Document Reference:
 *  - See report section "2.2.1 Service Locator Pattern (Additional)"
 ***************************************************************/
/// <summary>
/// 服务定位器 - 提供统一的服务注册和查找机制
/// 优势：
/// 1. 解耦：不直接依赖具体类，通过接口访问服务
/// 2. 可测试：可以注册Mock对象进行单元测试
/// 3. 可替换：可以轻松替换服务实现
/// 4. 集中管理：所有服务在一处注册和管理
/// </summary>
public class ServiceLocator
{
    private static ServiceLocator instance;
    private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

    /// <summary>
    /// 单例实例（服务定位器本身使用单例，但它管理的服务可以有不同的生命周期）
    /// </summary>
    public static ServiceLocator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ServiceLocator();
            }
            return instance;
        }
    }

    /// <summary>
    /// 注册单例服务
    /// </summary>
    /// <typeparam name="T">服务接口类型</typeparam>
    /// <param name="service">服务实例</param>
    /// <param name="overwrite">是否覆盖已存在的服务</param>
    public void RegisterSingleton<T>(T service, bool overwrite = false)
    {
        Type serviceType = typeof(T);

        if (services.ContainsKey(serviceType) && !overwrite)
        {
            Debug.LogWarning($"[ServiceLocator] Service {serviceType.Name} already registered. Use overwrite=true to replace.");
            return;
        }

        services[serviceType] = service;
        Debug.Log($"[ServiceLocator] Registered singleton service: {serviceType.Name}");
    }

    /// <summary>
    /// 获取服务实例
    /// </summary>
    /// <typeparam name="T">服务接口类型</typeparam>
    /// <returns>服务实例，如果未找到返回default(T)</returns>
    public T Get<T>()
    {
        Type serviceType = typeof(T);

        if (services.ContainsKey(serviceType))
        {
            return (T)services[serviceType];
        }

        // 未找到服务
        Debug.LogWarning($"[ServiceLocator] Service {serviceType.Name} not found. Please register it first.");
        return default(T);
    }

    /// <summary>
    /// 清空所有服务（通常在场景切换时调用）
    /// </summary>
    public void Clear()
    {
        services.Clear();
        Debug.Log("[ServiceLocator] All services cleared.");
    }

    /// <summary>
    /// 重置服务定位器（测试用）
    /// </summary>
    public static void Reset()
    {
        if (instance != null)
        {
            instance.Clear();
            instance = null;
        }
    }
}
/***************************************************************
 * End
 ***************************************************************/

