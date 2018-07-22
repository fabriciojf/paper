const state = {
  openedFiltersMenu: false,
  visible: false
}

const getters = {
  openedMenu: state => state.openedFiltersMenu,
  visible: state => state.visible
}

const mutations = {
  setMenuVisible (state, newState) {
    state.visible = newState
  },

  openMenu (state) {
    state.openedFiltersMenu = true
  },

  closeMenu (state) {
    state.openedFiltersMenu = false
  },

  changeMenuState (state) {
    state.openedFiltersMenu = !state.openedFiltersMenu
  }
}

export default {
  state,
  getters,
  mutations,
  namespaced: true
}
