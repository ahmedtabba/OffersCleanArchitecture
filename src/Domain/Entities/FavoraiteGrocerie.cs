namespace Offers.CleanArchitecture.Domain.Entities;

public class FavoraiteGrocery : BaseAuditableEntity
{
    public FavoraiteGrocery() :base()
    {
        
    }
    public string UserId { get; set; } = null!;
    public Guid GroceryId { get; set; } 
    public  virtual Grocery Grocery { get; set; } = new Grocery();


}

/*
 Id
 UserId
 GroceryId
 


Grocery => List of users

User => list of grocery
 */
