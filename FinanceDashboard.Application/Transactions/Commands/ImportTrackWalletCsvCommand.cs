using MediatR;

namespace FinanceDashboard.Application.Transactions.Commands;

public record ImportTrackWalletCsvCommand(Stream FileStream) : IRequest<int>;