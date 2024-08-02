# Integration Project


<p>
  <a href="https://opensource.org/licenses/MIT">
    <img alt="License: MIT" src="https://img.shields.io/badge/License-MIT-blue.svg">
  </a>
  <br/>
  <img src="./images/hepsiburada.png" width="48">
  <img src="./images/trendyol.webp" width="90">
  <img src="./images/sipay.jpeg" width="48">
  <img src="./images/paynet.jpg" width="90">
</p>

**Synchronize Your Code Universe**

Enhance your integration workflows with the Integration Library, specifically tailored for eCommerce marketplaces and payment gateways.

## Build Status

&nbsp; | `status` | `nuget`
--- | --- | --- 
**Integration.Hub** | ![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/onurkanbakirci/Integration/integration-hub.yml) | ![Nuget](https://img.shields.io/nuget/dt/Integration.Hub?color=green&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FIntegration.Hub) ![Nuget](https://img.shields.io/nuget/v/Integration.Hub?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FIntegration.Hub)
**Integration.Marketplaces.Trendyol** | ![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/onurkanbakirci/Integration/trendyol-integration.yml) |  ![Nuget](https://img.shields.io/nuget/dt/Integration.Marketplaces.Trendyol?color=green&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FIntegration.Marketplaces.Trendyol) ![Nuget](https://img.shields.io/nuget/v/Integration.Marketplaces.Trendyol?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FIntegration.Marketplaces.Trendyol)
**Integration.Marketplaces.Hepsiburada** | ![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/onurkanbakirci/Integration/hepsiburada-integration.yml) | ![Nuget](https://img.shields.io/nuget/dt/Integration.Marketplaces.Hepsiburada?color=green&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FIntegration.Marketplaces.Hepsiburada) ![Nuget](https://img.shields.io/nuget/v/Integration.Marketplaces.Hepsiburada?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FIntegration.Marketplaces.Hepsiburada)
**Integration.PaymentGateways.Paynet** | ![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/onurkanbakirci/Integration/paynet-integration.yml) |  ![Nuget](https://img.shields.io/nuget/dt/Integration.PaymentGateways.Paynet?color=green&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FIntegration.PaymentGateways.Paynet) ![Nuget](https://img.shields.io/nuget/v/Integration.PaymentGateways.Paynet?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FIntegration.PaymentGateways.Paynet)
**Integration.PaymentGateways.Sipay** | ![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/onurkanbakirci/Integration/sipay-integration.yml) |  ![Nuget](https://img.shields.io/nuget/dt/Integration.PaymentGateways.Sipay?color=green&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FIntegration.PaymentGateways.Sipay) ![Nuget](https://img.shields.io/nuget/v/Integration.PaymentGateways.Sipay?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FIntegration.PaymentGateways.Sipay)

## Table of contents

- [Integration Project](#integration-project)
  - [Build Status](#build-status)
  - [Table of contents](#table-of-contents)
  - [Introduction](#introduction)
  - [How to use](#how-to-use)
    - [Trendyol](#trendyol)
    - [Hepsiburada](#hepsiburada)
    - [Paynet](#paynet)
    - [Sipay](#sipay)


## Introduction

Enhance your integration workflows with the Integration Library, specifically tailored for eCommerce marketplaces and payment gateways. This library is designed to streamline and facilitate the integration process, providing seamless connectivity to a wide range of eCommerce platforms and payment services. Whether you're managing online stores, processing transactions, or integrating various eCommerce solutions, the Integration Library is a valuable resource for efficient and effective integration of these key systems and services.

## How to use

### Trendyol

```
dotnet add package Integration.Marketplaces.Trendyol --version 1.0.1
```

```c#
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Helpers;

var trendyolProductIntegration = new TrendyolProductIntegration(
        supplierId: "supplierId",
        apiKey: "apiKey",
        apiSecret: "apiSecret",
        isInProduction: false,
        entegratorFirm: "entegratorFirm");

//Get All Categories
var categories = await trendyolProductIntegration.GetCategoryTreeAsync();

//Get All Brands
var brands = await trendyolProductIntegration.GetBrandsAsync();

//Filter products
var productFilter = new ProductFilterBuilder()
    .AddApprovalStatus(true)
    .AddBarcode("barcode")
    .AddStartDate(0)
    .AddEndDate(0)
    .AddPage(1)
    .AddSize(10)
    .AddSupplierId(0)
    .Build();

var products = await trendyolProductIntegration.FilterProductsAsync(productFilter);
```

### Hepsiburada

```
dotnet add package Integration.Marketplaces.Hepsiburada --version 1.0.1
```

```c#
using Integration.Marketplaces.Hepsiburada.Infrastructure.ProductIntegration;

var hepsiburadaProductIntegration = new HepsiburadaProductIntegration(
        username: "username",
        password: "password",
        isInProduction: false);

//Get All Categories
var categories = await hepsiburadaProductIntegration.GetCategoriesAsync();

//Get Category Attributes
var categoryAttributes = await hepsiburadaProductIntegration.GetCategoryAttributesAsync(categoryId: 80844002);

// Get Category Attribute Values
var categoryAttributeValues = await hepsiburadaProductIntegration.GetCategoryAttributeValuesAsync(categoryId: 80844002, attributeId: "gram");
```

### Paynet

```
dotnet add package Integration.PaymentGateways.Paynet --version 1.0.0
```

```c#
using Integration.PaymentGateways.Paynet.Infrastructure.PaymentIntegration.Models.Request;

var paynetPaymentIntegration = new PaynetPaymentIntegration(
    secretKey: "secretKey",
    isInProduction: false);

// Get installments
var installments = await paynetPaymentIntegration.CheckInstallmentAsync(new CheckInstallmentRequestModel(bin: "bin", amount: 100));

// Non secure payment
var nonSecurePaymentResponse = await paynetPaymentIntegration.NonSecurePaymentAsync(new NonSecurePaymentRequestModel(
    amount: "100",
    referenceNo: "referenceNo",
    domain: "domain",
    cardHolder: "cardHolder",
    pan: "pan",
    month: "month",
    year: "year",
    cvc: "cvc",
    instalment: 1));

// Secure payment
// 1. Get secure payment initial
var securePaymentInitialResponse = await paynetPaymentIntegration.SecurePaymentInitialAsync(new SecurePaymentInitialRequestModel(
    amount: "100",
    orderRefNo: "orderRefNo",
    domain: "domain",
    cardHolder: "cardHolder",
    pan: "pan",
    month: 1,
    year: 2021,
    cvc: "cvc",
    returnUrl: "returnUrl",
    instalment: 1));

// 2. Show 3ds html content to user
Console.WriteLine(securePaymentInitialResponse.HtmlContent);

// 3. After successfull 3d confirmation, secure payment charge
var securePaymentChargeRequest = await paynetPaymentIntegration.SecurePaymentChargeAsync(new SecurePaymentChargeRequestModel(
    sessionId: securePaymentInitialResponse.SessionId,
    tokenId: securePaymentInitialResponse.TokenId));

// Cancel payment
var cancelResponse = await paynetPaymentIntegration.CancelAsync(new CancellationRequestModel(
    xactId: "xactId",
    succeedUrl: "succeedUrl"
));

// Refund payment
var refundResponse = await paynetPaymentIntegration.RefundAsync(new RefundRequestModel(
    xactId: "xactId",
    amount: "100",
    succeedUrl: "succeedUrl"
));
```

### Sipay

```
dotnet add package Integration.PaymentGateways.Sipay --version 1.0.0
```

```c#
using Integration.PaymentGateways.Sipay.Infrastructure.PaymentIntegration.Models.Request;
using Integration.PaymentGateways.Sipay.Infrastructure.PaymentIntegration.Models.Response;

var sipayPaymentIntegration = new SipayPaymentIntegration(
    merchantKey: "merchantKey",
    appKey: "appKey",
    appSecret: "appSecret",
    merchantId: 0,
    isInProduction: false
);

// Authorization is mandatory to further requests
await sipayPaymentIntegration.SetTokenAsync();

// Get installments
var installments = await sipayPaymentIntegration.CheckInstallmentsAsync(new CheckInstallmentRequestModel(
    creaditCard: "123456",
    amount: 100,
    currenyCode: "TRY"
));

// Non secure payment
var nonSecurePaymentResponse = await sipayPaymentIntegration.NonSecurePaymentAsync(new NonSecurePaymentRequestModel(
    ccHolderName: "John Doe",
    ccNo: "123456",
    expiryMonth: 12,
    expiryYear: 2022,
    cvv: 123,
    currencyCode: "TRY",
    installmentsNumber: 1,
    invoiceId: 1,
    invoiceDescription: "Invoice description",
    name: "John",
    surname: "Doe",
    total: 100,
    items: "items",
    cancelUrl: "https://cancelUrl.com",
    returnUrl: "https://returnUrl.com",
    hashKey: "hashKey",
    ip: "",
    orderType: 0
));

// Secure payment
// 1. Get secure payment hmtl content and show to user
var securePaymentInitialHtml = sipayPaymentIntegration.SecurePaymentInitial(new SecurePaymentInitialRequestModel(
    ccHolderName: "John Doe",
    ccNo: "123456",
    expiryMonth: 12,
    expiryYear: 2022,
    cvv: 123,
    currencyCode: "TRY",
    installmentsNumber: 1,
    invoiceId: "1",
    invoiceDescription: "Invoice description",
    name: "John",
    surname: "Doe",
    total: 100,
    items: "items",
    cancelUrl: "https://cancelUrl.com",
    returnUrl: "https://returnUrl.com",
    hashKey: "hashKey",
    orderType: 0,
    recurringPaymentNumber: 0,
    recurringPaymentCycle: "",
    recurringPaymentInterval: "",
    recurringWebHookKey: "",
    maturityPeriod: 0,
    paymentFrequency: 0
));

// 2. Show 3ds html content to user
Console.WriteLine(securePaymentInitialHtml);

// 3. After successfull 3d confirmation, secure payment charge
var securePaymentChargeRequest = await sipayPaymentIntegration.SecurePaymentChargeAsync(new SecurePaymentChargeRequestModel(
    invoiceId: "1",
    orderId: "1",
    status: "1",
    hashKey: ""));


// Cancel payment
var cancelResponse = await sipayPaymentIntegration.CancelAsync(new CancellationRequestModel(
    invoiceId: "",
    hashKey: "",
    refundTransactionId: "",
    refundWebhookKey: ""
));

// Refund payment
var refundResponse = await sipayPaymentIntegration.RefundAsync(new RefundRequestModel(
    invoiceId: "",
    amount: 100,
    hashKey: "",
    refundTransactionId: "",
    refundWebhookKey: ""
));
```
