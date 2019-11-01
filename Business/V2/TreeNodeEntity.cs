using System;
using System.Collections.Specialized;

namespace Kesco.Lib.Win.Data.Business.V2
{
	public abstract class TreeNodeEntity:Entity
	{
		public string _L			{get{return GetProp("l");}}
		public string _R			{get{return GetProp("r");}}
		public string _Parent		{get{return GetProp("parent");}		set{SetProp("parent",value);}}
		public string _Children		{get{if (!props.ContainsKey("children")) LoadChildren(); return props["children"];}}
		public string _Parents		{get{if (!props.ContainsKey("parents")) LoadParents(); return props["parents"];}}
	
		public int L	{get{ return _L.Length==0?0:int.Parse(_L);}}
		public int R	{get{ return _R.Length==0?0:int.Parse(_R);}}
		
		public void GetAllChildren(StringCollection col)
		{
			TreeNodeEntity child;
			StringCollection children = Str2Collection(_Children);
			foreach(string _child in children)
			{
				if(col.Contains(_child)) continue;
				
				col.Add(_child);
				child = (TreeNodeEntity)Activator.CreateInstance(GetType(),new object[]{_child});
				child.GetAllChildren(col);
			}
		}

		public void GetAllParent(StringCollection col)
		{
			TreeNodeEntity parent;
			StringCollection parents = Str2Collection(_Parents);
			foreach(string _parent in parents)
			{
				if(col.Contains(_parent)) continue;
				
				col.Add(_parent);
				parent = (TreeNodeEntity)Activator.CreateInstance(GetType(),new object[]{_parent});
				parent.GetAllParent(col);
			}
		}
		
		public bool ChildOf(string _rc)
		{
			if(_Parent.Length==0)
                return false;
			
			if(_Parent.Equals(_rc))
                return true;
		
            return TreeNodeParent.ChildOf(_rc);
		}

		public abstract TreeNodeEntity TreeNodeParent{get;}
		public abstract TreeNodeEntity[] TreeNodeChildren{get;}

		public abstract void LoadChildren();
		public abstract void LoadParents();

		public TreeNodeEntity(string id, string prefix, DBModule module):base(id,prefix,module)
		{
			
		}
	}
}
