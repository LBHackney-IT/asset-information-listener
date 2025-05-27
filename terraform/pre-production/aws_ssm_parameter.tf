resource "aws_ssm_parameter" "tenure_api_url" {
  name  = "/housing-tl/pre-production/tenure-api-url"
  type  = "String"
  value = "to_be_set_manually"

  lifecycle {
    ignore_changes = [
      value,
    ]
  }
}

resource "aws_ssm_parameter" "tenure_api_token" {
  name  = "/housing-tl/pre-production/tenure-api-token"
  type  = "String"
  value = "to_be_set_manually"

  lifecycle {
    ignore_changes = [
      value,
    ]
  }
}

resource "aws_ssm_parameter" "account_api_url" {
  name  = "/housing-finance/pre-production/account-api-url"
  type  = "String"
  value = "to_be_set_manually"

  lifecycle {
    ignore_changes = [
      value,
    ]
  }
}

resource "aws_ssm_parameter" "account_api_token" {
  name  = "/housing-tl/pre-production/account-api-token"
  type  = "String"
  value = "to_be_set_manually"

  lifecycle {
    ignore_changes = [
      value,
    ]
  }
}
