const state = {
  openedUserMenu: false,
  visible: false
}

const getters = {
  openedMenu: state => state.openedUserMenu,
  visible: state => state.visible
}

const mutations = {
  setMenuVisible (state, newState) {
    state.visible = newState
  },

  openMenu (state) {
    state.openedUserMenu = true
  },

  closeMenu (state) {
    state.openedUserMenu = false
  },

  changeMenuState (state) {
    state.openedUserMenu = !state.openedUserMenu
  }
}

export default {
  state,
  getters,
  mutations,
  namespaced: true
}
