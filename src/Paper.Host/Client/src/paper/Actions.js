export default class Actions {

  constructor (options) {
    this.store = options.store
  }

  get all () {
    var actions = []
    var entity = this.store.getters.entity
    if (entity && entity.actions) {
      actions = entity.actions.filter(action => action.name !== 'filters')
    }
    return actions
  }

  getAction (name) {
    var entity = this.store.getters.entity
    var action = null
    if (entity && entity.actions) {
      action = entity.getActionByName(name)
    }
    return action
  }

  getActions (entities) {
    if (!entities || !this.hasActions()) {
      return []
    }
    var commomActions = this._getDiffActions(entities)
    entities.forEach(entity => {
      if (!entity.hasAction()) {
        commomActions = []
        return
      }
      var keys = Object.keys(commomActions)
      var diffs = keys.filter(diff =>
        entity.actions.filter(action => action.name === diff).length === 0
      )
      diffs.forEach(diff => {
        delete commomActions[diff]
      })
    })
    return Object.values(commomActions)
  }

  getActionFields (entities, actionName) {
    if (!entities || !this.hasActions()) {
      return []
    }
    var actions = entities.filter(entity => {
      if (entity.actions) {
        return entity.actions.map(action => action.name === actionName)
      }
    })
    if (!actions) {
      return []
    }
    actions = actions.map(entity => entity.actions)
    var commomFields = this._getDiffFields(entities, actionName)
    actions.forEach(action => {
      action = action[0]
      var keys = Object.keys(commomFields)
      var diffs = keys.filter(diff => action.fields.filter(field => field.name === diff).length === 0)
      diffs.forEach(diff => {
        delete commomFields[diff]
      })
    })
    return Object.values(commomFields)
  }

  hasSubEntitiesActions () {
    var exist = this.store.getters.entity.entities.filter(entity => entity.hasAction())
    return exist
  }

  hasActions () {
    var entity = this.store.getters.entity
    return (entity && entity.actions) !== undefined
  }

  getTitle (action) {
    if (action !== null && action.title !== undefined && action.title.length > 0) {
      return action.title
    }
    if (action !== null && action.name !== undefined && action.name.length > 0) {
      return action.name
    }
    return ''
  }

  _getDiffActions (entities) {
    var flags = []
    entities.forEach(entity => {
      if (entity.hasAction()) {
        entity.actions.forEach(action => {
          flags[action.name] = action
        })
      }
    })
    return flags
  }

  _getDiffFields (entities, actionName) {
    var flags = []
    entities.forEach(entity => {
      if (entity.hasAction()) {
        var action = entity.actions.find(action => action.name === actionName)
        if (action) {
          action.fields.forEach(field => {
            flags[field.name] = field
          })
        }
      }
    })
    return flags
  }
}
