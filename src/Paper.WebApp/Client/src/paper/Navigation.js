export default class Navigation {

  constructor (options, actions) {
    this.store = options.store
    this.actions = actions
    this.openedRightMenu = this.store.getters['navigation/openedRightMenu']
  }

  get links () {
    var items = []
    var entity = this.store.getters.entity
    if (entity && entity.links) {
      items = entity.links.filter(
        item => item.rel.indexOf('self') &&
                item.rel.indexOf('next') &&
                item.rel.indexOf('previous') &&
                item.rel.indexOf('first')
      )
    }
    return items
  }

  hasLinks () {
    var entity = this.store.getters.entity
    if (entity && entity.links) {
      return this.links.length > 0
    }
    return false
  }

  showRightMenu () {
    var hasLinkOrAction = this.actions.hasActions() || this.hasLinks()
    var rightMenuVisible = this.store.getters['navigation/rightMenuVisible']
    return hasLinkOrAction && rightMenuVisible
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
