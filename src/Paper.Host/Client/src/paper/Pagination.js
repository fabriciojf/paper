export default class Pagination {

  constructor (options, requester, entity) {
    this.store = options.store
    this.requester = requester
    this.entity = entity
  }

  get previousLink () {
    var link = this.entity.getLinkByRel('previous')
    if (link) {
      return link.href
    }
  }

  get nextLink () {
    var link = this.entity.getLinkByRel('next')
    if (link) {
      return link.href
    }
  }

  get firstLink () {
    var link = this.entity.getLinkByRel('first')
    if (link) {
      return link.href
    }
  }

  showPrevious () {
    return this.entity.hasLinkByRel('previous')
  }

  showNext () {
    return this.entity.hasLinkByRel('next')
  }

  showFirst () {
    return this.entity.hasLinkByRel('first')
  }

  goToFirstPage () {
    this.requester.redirectToPage(this.firstLink)
  }

  goToNextPage () {
    this.requester.redirectToPage(this.nextLink)
  }

  goToPreviousPage () {
    this.requester.redirectToPage(this.previousLink)
  }

  getLink (page) {
    var link = this.entity.getLinkByRel(page)
    if (link) {
      return link.href
    }
    return '#'
  }

}
