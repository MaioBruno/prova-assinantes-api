using AssinantesApi.DTOs;
using AssinantesApi.Entities;

namespace AssinantesApi.Factories
{
    public interface IAssinanteFactory
    {
        Assinante Criar(AssinanteCreateDTO dto);
    }
}