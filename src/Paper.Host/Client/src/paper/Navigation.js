export default class Navigation {

  constructor (options, entity, actions) {
    this.store = options.store
    this.entity = entity
    this.actions = actions
    this.openedRightMenu = this.store.getters['navigation/openedRightMenu']
  }

  get links () {
    var links = this.entity.links
    if (links) {
      var items = links.filter(
        item => item.rel.indexOf('self') &&
                item.rel.indexOf('next') &&
                item.rel.indexOf('previous') &&
                item.rel.indexOf('first')
      )
      return items
    }
  }

  hasLinks () {
    var hasLinks = this.links && this.links.length > 0
    return hasLinks
  }

  showRightMenu () {
    var rightMenuVisible = this.store.getters['navigation/rightMenuVisible']
    return rightMenuVisible
  }

  openRightMenu () {
    this.store.commit('navigation/openRightMenu')
  }

  closeRightMenu () {
    this.store.commit('navigation/closeRightMenu')
  }

  setRightMenuVisible (visible) {
    this.store.commit('navigation/setRightMenuVisible', visible)
  }

  changeRightMenuState () {
    this.store.commit('navigation/changeRightMenuState')
  }

}
