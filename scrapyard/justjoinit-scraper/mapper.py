SOURCE_SITE_JUST_JOIN_IT = 0


def map_offers(offers: list[dict]) -> list[dict]:
    return [map_offer(offer) for offer in offers]


def map_offer(offer: dict) -> dict:
    return {
        "Id": offer["guid"],
        "Slug": offer["slug"],
        "Title": offer["title"],
        "CompanyName": offer["companyName"],
        "SourceSite": SOURCE_SITE_JUST_JOIN_IT,
        "Skills": offer.get("requiredSkills"),
        "WorkplaceTypes": _workplace_types(offer),
        "ExperienceLevels": _experience_levels(offer),
        "Locations": _locations(offer),
        "Salaries": _salaries(offer),
        "PublishedAt": offer.get("publishedAt"),
        "LogoUrl": offer.get("companyLogoThumbUrl"),
    }


def _workplace_types(offer: dict) -> list[str] | None:
    workplace_type = offer.get("workplaceType")
    if workplace_type is None or not workplace_type.strip():
        return None
    return [workplace_type]


def _experience_levels(offer: dict) -> list[str] | None:
    experience_level = offer.get("experienceLevel")
    if experience_level is None or not experience_level.strip():
        return None
    return [experience_level]


def _locations(offer: dict) -> list[str] | None:
    cities = [location.get("city") for location in offer.get("multilocation") or []]
    cities.append(offer.get("city"))
    unique = list(dict.fromkeys(city for city in cities if city and city.strip()))
    return unique or None


def _salaries(offer: dict) -> list[dict] | None:
    employment_types = offer.get("employmentTypes")
    if employment_types is None:
        return None
    return [
        {
            "From": employment_type.get("from"),
            "To": employment_type.get("to"),
            "Currency": employment_type.get("currency") or "",
            "ContractType": employment_type.get("type"),
        }
        for employment_type in employment_types
    ]
