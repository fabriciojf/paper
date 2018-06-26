export default class State {

  constructor (options) {
    this.store = options.store
  }

  get selection () {
    return this.store.getters['selection/isActive']
  }

  enableSelectionState () {
    this.store.commit('selection/changeSelectionState', true)
  }

  disableSelectionState () {
    this.store.commit('selection/changeSelectionState', false)
  }

  changeSelectionState (status) {
    this.store.commit('selection/changeSelectionState', status)
  }

}
