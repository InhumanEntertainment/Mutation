// Swarming Prototype //
// Move away from every other entity //
// Creat list of previous velocities //
/*
public class Entity
{
	public Vector3 Velocity;
	public Vector3 Acceleration;
	public Vector3 RandomAcceleration;
	public float Radius = 5;
	public float Force = 10;
	
	//============================================================================================================================================//
    void OnDrawGizmos()
	{
		Gizmos.DrawSphere(transform.position, Radius);
	}
}

public class EntityManager
{
	public GameObject EntityPrefab;
	public List<Entity> Entities = new List<Entity>();
	
	//============================================================================================================================================//
    public Entity CreateEntity()
	{
		Entity entity = Instatiate(EntityPrefab);
		Entities.Add(entity);
	}
	
	//============================================================================================================================================//
    void Update()
	{
		Vector3[] PreviousVelocity = new Vector3[Entities.Count]
		
		for(int i=0; i < Entities.Count; i++)
		{
			PreviousVelocity[i] = Entities[i].rigidBody.Velocity;
		}
		
		// Combine All Velocities //
		// if entity is not self and is within Radius, Add Force //
		for(int i=0; i < Entities.Count; i++)
		{
			for(int c=0; c < Entities.Count; c++)
			{
				if(i != c)
				{
					Vector3 dir = Entities[c] - Entities[i];
					float length = dir.length;
					
					if(length < radius)
					{
						float strength = 1f - (length / radius);
					
						Vector3 force = dir * strength;
						Entities[c].rigibody.Velocity += force;
					}
				}
			}
			
			Ent = Entities[i].rigidBody.Velocity;
		}
	}
}
*/