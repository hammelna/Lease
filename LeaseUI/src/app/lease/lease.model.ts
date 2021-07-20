export interface LeaseModel {
    name: string,
    startDate: Date,
    endDate: Date,
    paymentAmount: number,
    numberOfPayments: number,
    interestRate: number
}