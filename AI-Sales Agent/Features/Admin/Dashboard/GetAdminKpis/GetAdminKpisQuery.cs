using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminKpis;

public record GetAdminKpisQuery : IRequest<AdminKpiResponse>;
