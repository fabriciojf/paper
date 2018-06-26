const state = {
  entity: null
}

const mutations = {
  setEntity (state, entity) {
    state.entity = entity
  }
}

const getters = {
  blueprint: state => state.entity
}

export default {
  state,
  getters,
  mutations,
  namespaced: true
}
