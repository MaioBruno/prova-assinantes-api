using AssinantesApi.DTOs;
using AssinantesApi.Entities;

namespace AssinantesApi.Factories
{
    public class AssinanteFactory : IAssinanteFactory
    {
        public Assinante Criar(AssinanteCreateDTO dto)
        {
            return new Assinante(
                dto.NomeCompleto,
                dto.Email,
                dto.DataInicioAssinatura,
                dto.Plano,
                dto.ValorMensal
            );
        }
    }
}