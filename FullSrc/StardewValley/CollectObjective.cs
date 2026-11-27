using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Netcode;

namespace StardewValley;

public class CollectObjective : OrderObjective
{
	[XmlElement("acceptableContextTagSets")]
	public NetStringList acceptableContextTagSets = new NetStringList();

	public override void Load(SpecialOrder order, Dictionary<string, string> data)
	{
		if (data.ContainsKey("AcceptedContextTags"))
		{
			acceptableContextTagSets.Add(order.Parse(data["AcceptedContextTags"]));
		}
	}

	public override void InitializeNetFields()
	{
		base.InitializeNetFields();
		base.NetFields.AddFields(acceptableContextTagSets);
	}

	protected override void _Register()
	{
		base._Register();
		SpecialOrder order = _order;
		order.onItemCollected = (Action<Farmer, Item>)Delegate.Combine(order.onItemCollected, new Action<Farmer, Item>(OnItemShipped));
	}

	protected override void _Unregister()
	{
		base._Unregister();
		SpecialOrder order = _order;
		order.onItemCollected = (Action<Farmer, Item>)Delegate.Remove(order.onItemCollected, new Action<Farmer, Item>(OnItemShipped));
	}

	public virtual void OnItemShipped(Farmer farmer, Item item)
	{
		foreach (string acceptableContextTagSet in acceptableContextTagSets)
		{
			bool flag = false;
			string[] array = acceptableContextTagSet.Split(',');
			foreach (string obj in array)
			{
				bool flag2 = false;
				string[] array2 = obj.Split('/');
				foreach (string text in array2)
				{
					if (item.HasContextTag(text.Trim()))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				IncrementCount(item.Stack);
				break;
			}
		}
	}
}
