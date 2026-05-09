
using System.Linq.Expressions;
using System.Reflection;
using HarmonyLib;
using NeoModLoader.AndroidCompatibilityModule;
using NeoModLoader.constants;
using NeoModLoader.utils;
using UnityEngine;
using Object = UnityEngine.Object;
public class ObjectPoolGenericMono<T> where T : WrappedBehaviour
{
    private readonly List<T> _elements_total = new List<T>();

	public readonly Queue<T> _elements_inactive = new Queue<T>();

	private readonly T _prefab;

	private readonly Transform _parent_transform;

	public ObjectPoolGenericMono(T pPrefab, Transform pParentTransform)
	{
		_prefab = pPrefab;
		_parent_transform = pParentTransform;
	}

	public void clear(bool pDisable = true)
	{
		_elements_inactive.Clear();
		sortElements();
		foreach (T item in _elements_total)
		{
			release(item, pDisable);
		}
	}

	private void sortElements()
	{
		_elements_total.Sort((T a, T b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
	}

	public T getFirstActive()
	{
		return _elements_total[0];
	}

	public IReadOnlyList<T> getListTotal()
	{
		return _elements_total;
	}

	public void disableInactive()
	{
		foreach (T item in _elements_inactive)
		{
			if (item.gameObject.activeSelf)
			{
				item.gameObject.SetActive(value: false);
			}
		}
	}

	public T getNext()
	{
		T newOrActivate = getNewOrActivate();
		checkActive(newOrActivate);
		return newOrActivate;
	}

	private T getNewOrActivate()
	{
		T val;
		if (_elements_inactive.Count > 0)
		{
			val = _elements_inactive.Dequeue();
		}
		else
		{
			val = WrapperHelper.Instantiate(_prefab, _parent_transform);
			_elements_total.Add(val);
			val.name = typeof(T)?.ToString() + " " + _elements_total.Count + " " + val.transform.GetSiblingIndex();
		}
		return val;
	}

	public void release(T pElement, bool pDisable = true)
	{
		if (_parent_transform.gameObject.activeInHierarchy)
		{
			pElement.transform.SetAsLastSibling();
		}
		if (!_elements_inactive.Contains(pElement))
		{
			_elements_inactive.Enqueue(pElement);
		}
		if (pElement.gameObject.activeSelf && pDisable)
		{
			pElement.gameObject.SetActive(value: false);
		}
	}

	private void checkActive(T pElement)
	{
		if (!pElement.gameObject.activeSelf)
		{
			pElement.gameObject.SetActive(value: true);
		}
	}

	public int countTotal()
	{
		return _elements_total.Count;
	}

	public int countInactive()
	{
		return _elements_inactive.Count;
	}

	public int countActive()
	{
		return _elements_total.Count - _elements_inactive.Count;
	}

	public void resetParent()
	{
		foreach (T item in _elements_total)
		{
			resetParent(item);
		}
	}

	public void resetParent(T pElement)
	{
		if (_parent_transform.gameObject.activeInHierarchy)
		{
			pElement.transform.SetParent(_parent_transform);
		}
	}
}
namespace NeoModLoader.AndroidCompatibilityModule
{
	using static WrapperHelper;
	public static class WrapperHelper
	{
		internal static void Init()
		{
			Harmony harmony = new Harmony(Others.harmony_id);
			var patch = new HarmonyMethod(ClonePostfix);
			void Patch(string name)
			{
				harmony.Patch(AccessTools.Method(typeof(Object), name), null, patch);
			}
			harmony.Patch(
				AccessTools.Method(typeof(Object), nameof(Object.Instantiate),
					[typeof(Object), typeof(Transform), typeof(bool)]), null,  new HarmonyMethod(InstantiatePostfix));
			Patch(nameof(Object.Internal_InstantiateSingleWithParent));
			Patch(nameof(Object.Internal_InstantiateSingle));
			Patch(nameof(Object.Internal_CloneSingle));
		}
		public static void ClonePostfix(Object data, Object __result)
		{
			Resolve(data, __result);
		}
		public static void InstantiatePostfix(Object original, Object __result)
		{
			Resolve(original, __result);
		}
		public static void Resolve(Object orig, Object clone)
		{
			if (orig == null || clone == null)
			{
				return;
			}
			GameObject obj = orig.TryCast<GameObject>();
			if (obj != null)
			{
				WrapperResolver.ResolveInstantiate(obj, clone.Cast<GameObject>());
				return;
			}
			Component comp = orig.TryCast<Component>();
			if (comp != null)
			{
				WrapperResolver.ResolveInstantiate(comp.gameObject, clone.Cast<Component>().gameObject);
			}
		}
		public static WrappedAction GetWrappedMethod(Type type, string Method)
		{
			while (type != null)
			{
				var method = type.GetMethod(
					Method,
					BindingFlags.Instance |
					BindingFlags.Public |
					BindingFlags.NonPublic |
					BindingFlags.DeclaredOnly, Type.EmptyTypes);
				if (method != null)
					return method.AsDelegate<WrappedAction>();
				type = type.BaseType;
			}
			return null;
		}
		public static T Instantiate<T>(T original, Transform parent, bool worldPositionStays = true)
			where T : WrappedBehaviour
		{
			Il2CPPBehaviour il2cpp = Object.Instantiate(original.Wrapper, parent, worldPositionStays);
			return (T)il2cpp.WrappedBehaviour;
		}
		public static object GetWrappedComponent(GameObject Object, Type WrappedType)
		{
			foreach (Il2CPPBehaviour beh in Object.GetComponents<Il2CPPBehaviour>())
			{
				if (beh.WrappedBehaviour == null)
				{
					continue;
				}
				if (beh.WrappedType.IsAssignableTo(WrappedType))
				{
					return beh.WrappedBehaviour;
				}
			}
			return null;
		}

		public static int GetIndex(Component beh, GameObject obj)
		{
			var arr = obj.GetComponents(beh.GetType().C());
			int result = arr.GetIndex(beh);
			return result;
		}
		public static void AddChildren(Transform transform, List<Transform> children, out bool il2CppDetected)
		{
			il2CppDetected = false;
			foreach (Transform child in transform.GetChildren())
			{
				children.Add(child);
				AddChildren(child, children, out bool childDetected);
				il2CppDetected |= childDetected || child.HasComponent<Il2CPPBehaviour>();
			}
		}
		public static void AddChildren(Transform transform, List<Transform> children)
		{
			foreach (Transform child in transform.GetChildren())
			{
				children.Add(child);
				AddChildren(child, children);
			}
		}
	}
	public sealed class WrapperResolver : IDisposable
	{
		private static readonly Dictionary<Type, FieldInfo[]> fieldCache = new();
		private Il2CppSystem.Collections.Generic.Dictionary<Transform, Transform> origToClone; //use il2cpp dict to stop hash mismatches

		public static void ResolveInstantiate(GameObject orig, GameObject clone)
		{
			using var resolver = new WrapperResolver(orig, clone, out bool shouldResolve);
			if (shouldResolve) resolver.Resolve();
			resolver.Dispose();
		}

		public WrapperResolver(GameObject orig, GameObject clone, out bool shouldResolve)
		{
			var origTransforms = new List<Transform> { orig.transform };
			AddChildren(orig.transform, origTransforms, out bool detected);

			if (!detected && !orig.HasComponent<Il2CPPBehaviour>())
			{
				shouldResolve = false;
				return;
			}
			shouldResolve = true;
			
			var cloneTransforms = new List<Transform> { clone.transform };
			AddChildren(clone.transform, cloneTransforms);

			origToClone = new Il2CppSystem.Collections.Generic.Dictionary<Transform, Transform>(origTransforms.Count);
			for (int i = 0; i < origTransforms.Count; i++)
				origToClone[origTransforms[i]] = cloneTransforms[i];
		}
		public void Resolve()
		{
			foreach (var (origTransform, cloneTransform) in origToClone)
			{
				var origBehaviours = origTransform.GetComponents<Il2CPPBehaviour>();
				if (!origBehaviours.IsValid()) continue;

				var clonedBehaviours = cloneTransform.GetComponents<Il2CPPBehaviour>();
				for (int j = 0; j < origBehaviours.Length; j++)
					Clone(origBehaviours[j], clonedBehaviours[j]);
			}
		}

		public void Clone(Il2CPPBehaviour orig, Il2CPPBehaviour clone)
		{
			WrappedBehaviour beh = orig.WrappedBehaviour;
			if (beh == null) return;

			Type wrappedType = orig.WrappedType;
			WrappedBehaviour cloned = clone.CreateWrapperIfNull(wrappedType);

			foreach (var field in GetCachedFields(wrappedType))
			{
				var fieldObj = field.GetValue(beh);
				if (fieldObj == null) continue;

				Type type = field.FieldType;

				if (type == typeof(GameObject))
				{
					field.SetValue(cloned, ResolveGameObject((GameObject)fieldObj));
				}
				else if (type == typeof(Transform))
				{
					field.SetValue(cloned, ResolveGameObject(((Transform)fieldObj).gameObject).transform);
				}
				else if (typeof(Component).IsAssignableFrom(type))
				{
					var obj = (Component)fieldObj;
					field.SetValue(cloned,
						ResolveGameObject(obj.gameObject).GetComponent(type, GetIndex(obj, obj.gameObject)));
				}
				else if (typeof(WrappedBehaviour).IsAssignableFrom(type))
				{
					var obj = (WrappedBehaviour)fieldObj;
					var il2Cpp = (Il2CPPBehaviour)ResolveGameObject(obj.gameObject).GetComponent(typeof(Il2CPPBehaviour),
						GetIndex(obj.Wrapper, obj.gameObject));
					field.SetValue(cloned, il2Cpp.CreateWrapperIfNull(type));
				}
				else
				{
					field.SetValue(cloned, fieldObj);
				}
			}
		}

		private static FieldInfo[] GetCachedFields(Type type)
		{
			if (!fieldCache.TryGetValue(type, out var fields))
			{
				fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				fieldCache[type] = fields;
			}
			return fields;
		}

		public GameObject ResolveGameObject(GameObject orig)
		{
			return origToClone.TryGetValue(orig.transform, out var cloneTransform)
				? cloneTransform.gameObject
				: orig;
		}

		public void Dispose()
		{
			origToClone = null;
		}
	}
}