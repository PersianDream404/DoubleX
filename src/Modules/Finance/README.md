# Finance module

## Calculation policy

- Service commission is created only for Reception service rows whose status is `Completed`. The calculation base is the row's recorded `FinalAmount` (after discount), and the applicable commission rate and effective dates are read from the Personnel profile. Customer collection is not a prerequisite; an earning is a liability, not a cash transaction.
- Fixed salary is recorded once for the exact `EffectiveFrom`–`EffectiveTo` interval configured in the Personnel profile. The profile amount is the amount for that entire interval; this version does not prorate partial periods.
- Earning records snapshot the calculation inputs and use unique idempotency keys. A service row cannot create duplicate commission entries.
- Settlements are recorded as paid entries and allocated oldest-first to active earnings of the selected type. A serializable database transaction prevents concurrent over-settlement.
- Expenses are not physically deleted. Cancellation records its timestamp and reason, and cancelled expenses are excluded from active expense totals.

## Boundaries and limitations

- Finance reads Personnel and Reception through their application query contracts. It does not own personnel terms, service tariffs, reception payments, or inventory movements.
- The financial summary reports recorded personnel obligations, their allocations, and expenses. It does not report sales, collected revenue, material cost, or net profit because those measures are not available through the current module contracts.
- This version does not link settlements or expenses to CashRegister transactions. Such a link needs a retry-safe cross-module workflow; it must not be simulated as an atomic transaction across module databases.
- Changes or cancellations to a Reception service after its earning was calculated are not automatically reconciled. The current Reception contract has no financial-change event for Finance to consume.
