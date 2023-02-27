namespace ORM_1_21_
{
    /// <summary>
    ///Validating data entered into the database
    /// </summary>
    public interface IValidateDal<in T> where T : class
    {
      /// <summary>
      /// Call before insert or update
      /// </summary>
      /// <param name="item"></param>
        void Validate(T item);
    }
}