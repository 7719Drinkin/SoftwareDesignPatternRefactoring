using System;
using System.Collections.Generic;
using UnityEngine;

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
        private readonly Dictionary<Type, Func<object>> factories = new Dictionary<Type, Func<object>>();

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
        /// 注册工厂方法（每次Get时创建新实例）
        /// </summary>
        /// <typeparam name="T">服务接口类型</typeparam>
        /// <param name="factory">工厂方法</param>
        /// <param name="overwrite">是否覆盖已存在的工厂</param>
        public void RegisterFactory<T>(Func<T> factory, bool overwrite = false)
        {
            Type serviceType = typeof(T);

            if (factories.ContainsKey(serviceType) && !overwrite)
            {
                Debug.LogWarning($"[ServiceLocator] Factory for {serviceType.Name} already registered. Use overwrite=true to replace.");
                return;
            }

            factories[serviceType] = () => factory();
            Debug.Log($"[ServiceLocator] Registered factory for: {serviceType.Name}");
        }

        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <typeparam name="T">服务接口类型</typeparam>
        /// <returns>服务实例，如果未找到返回default(T)</returns>
        public T Get<T>()
        {
            Type serviceType = typeof(T);

            // 1. 优先查找单例服务
            if (services.ContainsKey(serviceType))
            {
                return (T)services[serviceType];
            }

            // 2. 查找工厂方法
            if (factories.ContainsKey(serviceType))
            {
                return (T)factories[serviceType]();
            }

            // 3. 未找到服务
            Debug.LogWarning($"[ServiceLocator] Service {serviceType.Name} not found. Please register it first.");
            return default(T);
        }

        /// <summary>
        /// 尝试获取服务实例
        /// </summary>
        /// <typeparam name="T">服务接口类型</typeparam>
        /// <param name="service">输出的服务实例</param>
        /// <returns>是否成功获取服务</returns>
        public bool TryGet<T>(out T service)
        {
            Type serviceType = typeof(T);

            if (services.ContainsKey(serviceType))
            {
                service = (T)services[serviceType];
                return true;
            }

            if (factories.ContainsKey(serviceType))
            {
                service = (T)factories[serviceType]();
                return true;
            }

            service = default(T);
            return false;
        }

        /// <summary>
        /// 检查服务是否已注册
        /// </summary>
        /// <typeparam name="T">服务接口类型</typeparam>
        /// <returns>是否已注册</returns>
        public bool IsRegistered<T>()
        {
            Type serviceType = typeof(T);
            return services.ContainsKey(serviceType) || factories.ContainsKey(serviceType);
        }

        /// <summary>
        /// 注销服务
        /// </summary>
        /// <typeparam name="T">服务接口类型</typeparam>
        public void Unregister<T>()
        {
            Type serviceType = typeof(T);
            
            if (services.Remove(serviceType))
            {
                Debug.Log($"[ServiceLocator] Unregistered service: {serviceType.Name}");
            }
            
            if (factories.Remove(serviceType))
            {
                Debug.Log($"[ServiceLocator] Unregistered factory: {serviceType.Name}");
            }
        }

        /// <summary>
        /// 清空所有服务（通常在场景切换时调用）
        /// </summary>
        public void Clear()
        {
            services.Clear();
            factories.Clear();
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

