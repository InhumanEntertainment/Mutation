namespace Inhuman
{
	public class Store
	{
		public Product[] Products;
		public void Initialize(){}
		public void GetProducts(){}
		public void BuyProduct(string product, int amount){}
		public void Restore(){}
		public void 
	}
	
	public class Social
	{
		public Achievement[] Achievements;
		public void SetAchievement(string name, int value){}
		public void GetAchievements();
		public void Restore();
	}
	
	public class Utilities
	{
		public void MessageBox(string title, string message){}
		
	}
	
	public class Build
	{
		public string Name;
		public Platform[] Platforms;
		public GameObject ScriptObject;
		public string ScriptMessage;
		Public Texture Icon;
		
	}
	
	public class BuildManager
	{
		public Build[] Builds;
		public string Platform;
		public void SetPlatform(){}
		public void BuildProject(){}
		
	}
	
	
}