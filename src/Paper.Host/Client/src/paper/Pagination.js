export default class Pagination {

  constructor (options, requester) {
    this.store = options.store
    this.requester = requester
  }

  previousLink () {
    return this.showPrevious ? this.store.state.entity.getLinkByRel('previous').href : '#'
  }

  nextLink () {
    return this.showPrevious ? this.store.state.entity.getLinkByRel('next').href : '#'
  }

  firstLink () {
    return this.showPrevious ? this.store.state.entity.getLinkByRel('first').href : '#'
  }

  showPrevious () {
    return this.store.state.entity && this.store.state.entity.hasLinkByRel('previous')
  }

  showNext () {
    return this.store.state.entity && this.store.state.entity.hasLinkByRel('next')
  }

  showFirst () {
    return this.store.state.entity && this.store.state.entity.hasLinkByRel('first')
  }

  goToFirstPage () {
    this.requester.redirectToPage(this.firstLink())
  }

  goToNextPage () {
    this.requester.redirectToPage(this.nextLink())
  }

  goToPreviousPage () {
    this.requester.redirectToPage(this.previousLink())
  }

  getLink (page) {
    switch (page) {
      case 'next':
        return this.nextLink
      case 'previous':
        return this.previousLink
      case 'first':
        return this.previousLink
      default:
        return '#'
    }
  }

}
