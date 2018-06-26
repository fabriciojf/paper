const state = {
  openedRightMenu: false,
  rightMenuVisible: false
}

const getters = {
  openedRightMenu: state => state.openedRightMenu,
  rightMenuVisible: state => state.rightMenuVisible
}

const mutations = {
  setRightMenuVisible (state, newState) {
    state.rightMenuVisible = newState
  },

  openRightMenu (state) {
    state.openedRightMenu = true
  },

  closeRightMenu (state) {
    state.openedRightMenu = false
  },

  changeRightMenuState (state) {
    state.openedRightMenu = !state.openedRightMenu
  }
}

export default {
  state,
  getters,
  mutations,
  namespaced: true
}
