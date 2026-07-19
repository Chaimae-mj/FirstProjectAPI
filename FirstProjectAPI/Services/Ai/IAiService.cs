using FirstProjectAPI.Dtos.Ai;

namespace FirstProjectAPI.Services.Ai
{
    public interface IAiService
    {
        /// <summary>
        /// L'apprenant pose une question libre, l'IA répond directement.
        /// </summary>
        Task<string> AskAsync(string question);

        /// <summary>
        /// L'IA explique le contenu d'un module (titre + modalités en base).
        /// </summary>
        Task<ExplainModuleResponseDto> ExplainModuleAsync(ExplainModuleDto dto);
    }
}