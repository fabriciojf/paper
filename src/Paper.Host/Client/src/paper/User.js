export default class User {

  constructor (options) {
    this.store = options.store
  }

  get user () {
    return {
      id: 1,
      name: 'Francine'
    }
  }

  openMenu () {
    this.store.commit('user/openMenu')
  }

  closeMenu () {
    this.store.commit('user/closeMenu')
  }

  changeMenuState () {
    this.store.commit('user/changeMenuState')
  }

}
