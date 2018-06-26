const state = {
  selectionState: false,
  selectedItems: []
}

const getters = {
  items: state => state.selectedItems,
  isActive: state => state.selectionState
}

const mutations = {

  changeSelectionState (state, status) {
    state.selectionState = status
  },

  setSelectedItems (state, items) {
    state.selectionState = items.length > 0
    state.selectedItems = items
  }

}

export default {
  state,
  mutations,
  getters,
  namespaced: true
}
