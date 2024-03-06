namespace SimsProject.Application.Interface
{
    public interface IService<T>
    {
        T GetById(int id);
    }
}
