# SmartFactoryManagementSystem

** Architectural &amp; Operational Specification

** 1. Architectural Overview &amp; System Summary

The SmartFactoryManagementSystem is a console-based enterprise simulation that models the
operations of an automated electronics manufacturing facility (producing smartphones, tablets,
computers, and headphones).

The system leverages a centralized orchestration pattern. The entry point (Program.cs)
initializes the primary orchestrator (Factory), seeds the application state with baseline personnel
records and machine hardware profiles, and yields execution flow control to the
MenuManagement subsystem. The user interface exposes operational vectors via a CLI loop,
translating user input into commands handled by the Factory orchestrator. The Factory then
delegates business logic to domain-specific subsystem managers (EmployeeManagement,
MachineManagement, ProductManagement) and domain models (workers/machines).
** 2. System Workflows &amp; Sequence Dynamics

Execution Pipeline: CLI Input -&gt; Factory Orchestrator -&gt; Subsystem Validation Managers -&gt;
Domain Model Execution
A. Production Lifecycle Workflow
MenuManagement (CLI Loop)
└── Factory.ManufactureProductWithMachine(operatorId, machineSerial,
productType, ...)
├── EmployeeManager.SearchEmployee() [Validates record existence &amp;
confirms MachineOperator subclass]
├── MachineManager.Search() [Validates machine structural state
&amp; availability]
├── MachineOperator.StartMachine() [Mutates state machine property to
State.Running]
├── Machine.Process(Product) [Executes cycle, decrements
component health metrics, simulates MTBF wear]
└── ProductManagement.AddProduct() [Appends instantiation of finalized
product to persistent Inventory]

B. Transactional Sales Workflow
MenuManagement (CLI Loop)
└── Factory.SellProductWithAgent(agentId, productId)
├── EmployeeManager.SearchEmployee() [Confirms agent existence &amp;
validates SalesAgent RBAC permissions]
├── ProductManagement.Search() [Queries Inventory state for stock
availability]
├── ProductManagement.SoldProduct() [Evicts product instance from stock
collection/repository]
└── Factory.ModifyBudget() [Applies ledger transaction
updating factory balance sheet (RON)]
3. Deployment and CLI Navigation Guide
Compilation &amp; Execution
 1. Open the project source root within a C#/.NET Core compatible IDE (e.g., Visual Studio,
JetBrains Rider, or VS Code).
 2. Ensure all compilation units (*.cs files) are scoped under the unified ElectronicsFactory
namespace.
 3. Open a terminal instance in the root directory and execute the .NET CLI command:
dotnet run
 4. Upon successful compilation, the console will block on a splash screen. Strike any key to
initialize the primary execution menu.
CLI Navigation Matrix
Input characters map directly to backend dispatch routines:
 &#39;1&#39; — Personnel Management Subsystem: Handles employee onboarding, offboarding, and
collection parsing.
 &#39;2&#39; — Hardware Infrastructure Subsystem: Controls machine provisioning, structural health
inspection, and component swaps.
 &#39;3&#39; — Inventory Management Subsystem: Queries and renders current warehouse stock
analytics.
 &#39;4&#39; — Production Orchestration Panel: Triggers automated fabrication pipelines, quality
assurance testing, and packaging loops.
 &#39;5&#39; — Fiscal Operations &amp; Reporting: Invokes sales processing via specialized agents or
triggers ledger valuations via accounting.
 &#39;6&#39; — Executive Summary Dashboard: Renders a high-level factory performance report
compiled by the Director.
 &#39;0&#39; — SIGTERM System Exit: Gracefully terminates execution loops and disposes of
memory assets.
Execution Note: When prompted for entity IDs or identifiers by the sub-menus, provide the
explicit primary keys or unique strings initialized inside Program.cs (e.g., Worker Names:
&quot;Mihai Ion&quot;, &quot;Elena Popa&quot;; Machine Serials: &quot;X100&quot;, &quot;Z90&quot;).
4. Domain Models &amp; Role-Based Access Control
(RBAC)
Every worker class extends a base employee type, encapsulating distinct operational
boundaries and system capabilities within the domain:
1. Director (Director)
 Domain Role: Executive Operations Overseer.
 Functional Scope: Holds exclusive execution rights to ReviewProductionStatistics.
Aggregates and returns analytical data regarding global personnel counts, hardware
infrastructure health, current inventory distribution, and real-time liquidity tracking.
2. Production Manager (ProductionManager)
 Domain Role: Operations Coordinator.
 Functional Scope: Supervises the operational floor. Responsible for scheduling workflows,
evaluating process efficiency metrics, and managing operational floor dispatching.
3. Machine Operator (MachineOperator)
 Domain Role: Hardware Runtime Handler.
 Functional Scope: Authorized to execute StartMachine. Acts as the mandatory execution
trigger for hardware state transitions. Production routines fail immediately if an active, valid
operator is not passed to the pipeline.
4. Engineer (Engineer)
 Domain Role: Predictive Diagnostics Specialist.
 Functional Scope: Evaluates asset metrics, inspects system components, and calculates
degradation tolerances to prevent critical faults or unexpected system downtime during
runtime blocks.
5. Technician (Technician)
 Domain Role: Remediation &amp; Component Maintenance Specialist.
 Functional Scope: Performs structural restorations and components replacements on units
with high wear factors or crashed states, returning hardware parameters back to baseline
limits.
6. Sales Agent (SalesAgent)
 Domain Role: Fiscal Liquidation Representative.
 Functional Scope: Holds exclusive privilege to call SellProductWithAgent. Mutates
inventory collections, handles asset de-allocation, and routes incoming cash flows directly
into the central ledger.
7. Accountant (Accountant)
 Domain Role: Financial Auditor &amp; General Ledger Assessor.
Functional Scope: Audits current production valuations, calculates operational overhead
amortizations, evaluates asset profitability curves, and compiles the real-time financial
balance sheet.
